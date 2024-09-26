// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory{ HostName = "localhost" };
//factory.Uri = new Uri("http://localhost:5672"); //Rabbitmq'yu docker üzerinden ayağa kaldırdık. Dockerda container'ın ip adresi 172.17.0.2'dir. 

var connection = factory.CreateConnection(); //Bağlantıyı sağladık.

var channel = connection.CreateModel(); //Yeni bir kanal oluşturduk.

//İlk parametre queue yani kuyruğun ismi
//ikinci parametre durable yani true false'a göre memoryde tutulup tutulmamasını belirler. True yaparsak fiziksel olarak kaydedilir rabbitmq restartlansa bile kuyruk kaybolmaz.
//üçüncü parametre exclusive yani true yaparsak burdaki kuyruğa bu projede oluşturduğumuz kanal ile bağlantı kurabilirim anlamına geliyor. Biz buradaki kuyruğa subscriber üzerinden farklı bir kanaldan bağlanacağımız için false yapıyoruz.
//dördüncü parametre autodelete bu parametrenin yaptığı işlem ise bu kuyruğa bağlı olan son subscriber da kuyruğa bağlantısını kapatırsa bu kuyruk otomatik olarak silinir.
channel.QueueDeclare("hello-queue", true, false, false);

//Foreach ile bir kerede 50 tane mesaj göndericez.
Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    string message = $"Message {x}";

    var messageBody = Encoding.UTF8.GetBytes(message); //Rabbitmq da mesajlar byte olarak gönderilir.

    //İlk parametre exchange kullanmadığımız için default exchange olarak geçer.
    //İkinci parametre routing key olarak kuyruk ismi verilir. 
    //Üçüncü parametre olarak basic propertyleri null geçtik.
    //Dördüncü parametre ôlarak byte dizisi olarak body'yi veririz.
    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

    Console.WriteLine($"Mesaj gönderilmiştir.: {message}");
});



Console.ReadLine();
