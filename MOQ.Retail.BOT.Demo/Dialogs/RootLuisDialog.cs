using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace MOQ.Retail.BOT.Demo.Dialogs
{
 

    [LuisModel("157282bb-273c-4ef1-8ab2-c2ea00b45971", "7af808e1d02d4dc1a759cd63c9b0f605")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        private const string EntityOrder = "Order";

        private const string EntityProduct = "Hotel";


        private IList<string> titleOptions = new List<string> { "“Very stylish, great stay, great staff”", "“good hotel awful meals”", "“Need more attention to little things”", "“Lovely small hotel ideally situated to explore the area.”", "“Positive surprise”", "“Beautiful suite and resort”" };

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Track")]
        public async Task Search(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to the Order tracker, How Can I help you Today? '{message.Text}'...");

            var orderQuery = new OrderQuery();

  

            var ordersFormDialog = new FormDialog<OrderQuery>(orderQuery, this.BuilOrdersForm, FormOptions.PromptInStart, result.Entities);

            context.Call(ordersFormDialog, this.ResumeAfterHotelsFormDialog);
        }

        [LuisIntent("ShowHotelsReviews")]
        //public async Task Reviews(IDialogContext context, LuisResult result)
        //{
        //    EntityRecommendation hotelEntityRecommendation;

        //    if (result.TryFindEntity(EntityHotelName, out hotelEntityRecommendation))
        //    {
        //        await context.PostAsync($"Looking for reviews of '{hotelEntityRecommendation.Entity}'...");

        //        var resultMessage = context.MakeMessage();
        //        resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
        //        resultMessage.Attachments = new List<Attachment>();

        //        for (int i = 0; i < 5; i++)
        //        {
        //            var random = new Random(i);
        //            ThumbnailCard thumbnailCard = new ThumbnailCard()
        //            {
        //                Title = this.titleOptions[random.Next(0, this.titleOptions.Count - 1)],
        //                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris odio magna, sodales vel ligula sit amet, vulputate vehicula velit. Nulla quis consectetur neque, sed commodo metus.",
        //                Images = new List<CardImage>()
        //                {
        //                    new CardImage() { Url = "https://upload.wikimedia.org/wikipedia/en/e/ee/Unknown-person.gif" }
        //                },
        //            };

        //            resultMessage.Attachments.Add(thumbnailCard.ToAttachment());
        //        }

        //        await context.PostAsync(resultMessage);
        //    }

        //    context.Wait(this.MessageReceived);
        //}

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! Try asking me things like 'search hotels in Seattle', 'search hotels near LAX airport' or 'show me the reviews of The Bot Resort'");

            context.Wait(this.MessageReceived);
        }

        private IForm<OrderQuery> BuilOrdersForm()
        {
            OnCompletionAsyncDelegate<OrderQuery> processOrderSearch = async (context, state) =>
            {
                var message = "Searching for the  Order";
                if (!string.IsNullOrEmpty(state.OrderID))
                {
                    message += $" in {state.OrderID}...";
                }
              

                await context.PostAsync(message);
            };

            return new FormBuilder<OrderQuery>()
                .Field(nameof(OrderQuery.OrderID), (state) => string.IsNullOrEmpty(state.OrderID))
                .OnCompletion(processOrderSearch)
                .Build();
        }

        private async Task ResumeAfterHotelsFormDialog(IDialogContext context, IAwaitable<OrderQuery> result)
        {
            try
            {
                var searchQuery = await result;

                var order = await this.GetOrderAsync(searchQuery);
                
                if(order.ProductName.Length>0)
                await context.PostAsync($"I found the order: {order.ID}");
                else
                    await context.PostAsync($"Unable to find the order: {order.ID}");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                    HeroCard heroCard = new HeroCard()
                    {
                        Title = order.ID,
                        Subtitle = $"{order.CustomerID} starts. {order.ProductName}",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url =null }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=hotels+in+" + HttpUtility.UrlEncode(order.ID)
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                

                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        private async Task<Order> GetOrderAsync(OrderQuery searchQuery)
        {


            var Order = new Order()
            {


                ID = $"{searchQuery.OrderID}",
                AmountPaid = 0,
                Balance=0,
                CustomerID="test",
                ProductName="test",
                Quantity=0,
                TotalPrice=0,
                UnitPrice=0
           
                };
            

 

            return Order;
        }
    }
}