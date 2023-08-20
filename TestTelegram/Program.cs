using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TestTelegram
{
    internal class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("ТОКЕН БОТА");
        private static ChatId testChat = "ID группы";
        static List<Users> userslist = new List<Users>();
        static int find = 0;
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            // Проверяем тип обновления
            if ((update.Type == Telegram.Bot.Types.Enums.UpdateType.Message) & (message.Text != null))
            {
                // Проверяем создана ли тема для этого пользователя в группе
                foreach (Users persons in userslist)
                {
                    if (message.From.Id == persons.Id) find++;
                    if (find > 0) break;
                }
                //если нет создаем тему и добавляем пользователя в список
                if (find == 0)
                {
                    string result = JsonConvert.SerializeObject(botClient.CreateForumTopicAsync(testChat, message.From.FirstName + " " + message.From.LastName)).ToString();
                    var root = JsonConvert.DeserializeObject<ResultFormCreate>(result);
                    await botClient.ForwardMessageAsync(testChat, message.Chat.Id, message.MessageId, root.Result.message_thread_id);
                    Users person = new Users();
                    person.MessageThreadId = root.Result.message_thread_id;
                    person.FirstName = message.From.FirstName;
                    person.LastName = message.From.LastName;
                    person.Id = message.From.Id;
                    userslist.Add(person);

                }
                //если сообщение из группы пересылем пользователю, который обратился к боту
                else if (message.Chat.Id == testChat)
                {
                    var a = userslist.Where(x => x.MessageThreadId == message.MessageThreadId).FirstOrDefault();
                    await botClient.SendTextMessageAsync(a.Id, message.Text);
                }
                //если сообщение от пользователя пересылаем его сообщение в тему созданную под него
                else
                {
                    var a = userslist.Where(x => x.Id == message.Chat.Id).FirstOrDefault();
                    await botClient.ForwardMessageAsync(testChat, message.Chat.Id, message.MessageId, a.MessageThreadId);
                }
                find = 0;
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        static void Main(string[] args)
        {
            // Добавляем администратора
            var admin = new Users();
            admin.FirstName = "ИМЯ АДМИНА";
            admin.LastName = "ФАМИЛИЯ АДМИНА";
            admin.Id = 0123465; //ID АДМИНА
            userslist.Add(admin);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}