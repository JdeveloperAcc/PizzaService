using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PizzaService.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace PizzaService.Web
{
    public class Program
    {
        // Global reference to the timer
        private static Timer myTimer = null;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            new DbFactory().Method<TodoDb>(builder, "OrderList");
            var app = builder.Build();

            if (myTimer == null)
            {
                int intervalMs = Cfg.Read(app.Configuration).IntervalMs;
                myTimer = new Timer(intervalMs);
                // Attach the Tick method to the Elapsed event
                myTimer.Elapsed += OnTimedEvent;
                // Enable the Timer
                myTimer.Enabled = true;
            }

            var todoItems = app.MapGroup("/");
            todoItems.MapGet("/", IndexPage);
            todoItems.MapGet("/orders", GetAllOrders);
            todoItems.MapGet("/orders/{id}", GetOrder);
            todoItems.MapGet("/orders/product/{name}", GetNamedOrder);
            todoItems.MapPost("/submit", CreateOrder);

            app.Run();

            static IResult IndexPage()
            {
                return Results.Text("<html><head/><body><h2>Pizza Service</h2></body></html>", "text/html", System.Text.Encoding.UTF8);
            }

            static async Task<IResult> GetAllOrders(TodoDb db)
            {
                return TypedResults.Ok(await new Agregator(db).SingletonManipulator.GetAggregatedResultsAsync());
            }

            static async Task<IResult> GetOrder(uint id, TodoDb db)
            {
                return await new Agregator(db).SingletonManipulator.GetDataItemAsync(id)
                    is Order todo
                        ? TypedResults.Ok(todo)
                        : TypedResults.NotFound();
            }

            static async Task<IResult> GetNamedOrder(string name, TodoDb db)
            {
                return await new Agregator(db).SingletonManipulator.GetDataNamedAsync(name)
                    is Order todo
                        ? TypedResults.Ok(todo)
                        : TypedResults.NotFound();
            }

            static async Task<IResult> CreateOrder(Order[] todos, TodoDb db)
            {

                int submittedQty = await new Agregator(db).SingletonManipulator.AddDataIncrementAsync(todos);
                return submittedQty > 0 ? TypedResults.Ok(
                            HashProvider.Calc(todos.Select(ord => new string(ord.Id.ToString())).ToArray(), submittedQty))
                    : TypedResults.BadRequest();
            }

            static async Task<bool> Synchronize(bool isFullSync, TodoDb db)
            {
                S2SFeedConsole s2s = new S2SFeedConsole();
                Agregator feed = new Agregator(db);

                var listed = await feed.SingletonManipulator.GetAggregatedResultsAsync();

                bool isSent = s2s.Commit(listed);
                bool result = s2s.CommitStatus(listed);

                // In case you subtract sent data server 2 server   
                if (isFullSync)
                {             
                    if (isSent)
                        await feed.SingletonManipulator.ConfirmDeductedAsync(listed);

                    if (result)
                        await feed.SingletonManipulator.CommitResultOkAsync();
                    else
                        await feed.SingletonManipulator.CommitResultErrorAsync();
                }
                              
                return result;
            }

            async void OnTimedEvent(Object source, ElapsedEventArgs e)
            {
                try
                {
                    // Use DbContext instance from a service pool.
                    // Do not dispose an app's singleton object!
                    var scope = app.Services.CreateScope();
                    TodoDb context = scope.ServiceProvider.GetRequiredService<TodoDb>();
                    await Synchronize(Cfg.Read(app.Configuration).IsExtendedSync, context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
        }
    }
}