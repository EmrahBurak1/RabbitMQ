// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

#region Exchange olmadan rabbitmq kullanımı. (Default exchange olarak geçiyor)
//var factory = new ConnectionFactory { HostName = "localhost" };

//var connection = factory.CreateConnection(); //Bağlantıyı sağladık.

//var channel = connection.CreateModel(); //Yeni bir kanal oluşturduk.

////İlk parametre queue yani kuyruğun ismi
////ikinci parametre durable yani true false'a göre memoryde tutulup tutulmamasını belirler. True yaparsak fiziksel olarak kaydedilir rabbitmq restartlansa bile kuyruk kaybolmaz.
////üçüncü parametre exclusive yani true yaparsak burdaki kuyruğa bu projede oluşturduğumuz kanal ile bağlantı kurabilirim anlamına geliyor. Biz buradaki kuyruğa subscriber üzerinden farklı bir kanaldan bağlanacağımız için false yapıyoruz.
////dördüncü parametre autodelete bu parametrenin yaptığı işlem ise bu kuyruğa bağlı olan son subscriber da kuyruğa bağlantısını kapatırsa bu kuyruk otomatik olarak silinir.
////channel.QueueDeclare("hello-queue", true, false, false); //Hem publisher hem subscriber tarafında kuyruk tanımlıyorsak ikisinin de parametreleri aynı olmalı.

////Burada da her bir subscriber'a kaç mesaj gelsin onu belirleyebiliyoruz. Örneğin 50 mesajımız var bunu 5 subscriber'a 10'ar 10'ar göndermesini sağlayabiliyoruz.
////İlk parametreye sıfır yazıyoruz yani herhangi bir boyuttaki mesajı gönderebiliriz anlamına geliyor.
////İkinci parametre ise her subscriber'a 1'er tane mesaj gelsin anlamına geliyor.
////Üçüncü parametre olan global false olursa her bir subscriber'a kaç adet gönderileceğini belirtir. Yani 2.parametreyi 5 yapıp 3.paramatre false olursa her bir subscriber'a 5'er 5'er gönder anlamına gelir true yaparsakta örneğin 2.parametre 6 ve 2 tane de subscriber var bunu 3'er 3'er bölürek her bir sub'a gönderir. Mesela true deyip 2. parametreye 3 yazarsak iki tane de sub'ımız varsa 2 yukardakine 1 aşağıdakine gönderir toplamı 3 olur.
//channel.BasicQos(0, 1, false);

//var consumer = new EventingBasicConsumer(channel);

////İlk parametremiz kuyruğun ismi olur.
////ikinci parametremiz olan autoAck parametresi true olursa bu mesaj doğruda işlense yanlışta işlence kuyruktan direkt siler. False yapılırsa kuyruktan silmez doğru bir şekilde işlerse biz silmesi için komut vericez anlamına gelir.
////üçüncü parametre ise consumer'dır.
//channel.BasicConsume("hello-queue", false, consumer);

////Bu event rabbitmq bu subscriber'a bir mesaj gönderdiğinde bu event fırlatılıyor.
////Yani gelen mesajları bu şekilde dinliyoruz.
//consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
//{
//    var message = Encoding.UTF8.GetString(e.Body.ToArray());

//    Thread.Sleep(1000); //Görevilmek için 1 saniyelik gecikme verdik.

//    Console.WriteLine("Gelen Mesaj:" + message);

//    //BasicConsume içinde autoAct ifadesini false yapınca otomatik olarak silmeyecek. Biz burada gelen mesajı işledikten sonra silmesini istiyoruz. O yüzden basci act ile yapabiliriz.
//    //İlk parametre DeliveryTag'ın anlamı ilgili mesajı artık silebiliriz anlamına geliyor. Yani bize ulaştırılan tag'ı rabbitmq'ya gönderiyoruz sonra hangi tag ile o mesaj bize ulaştırılmışsa ilgili mesajı bulup kuyruktan siliyor.
//    //İkinci parametre multiple, true dersek o anda memory de işlenmiş ama rabbitmq'ya gitmemiş başka mesajlar da varsa rabbitmq ya haber verir. False ise sadece ilgili mesajın durumunu rabbitmq ya bildiriyoruz.
//    channel.BasicAck(e.DeliveryTag,false);
//};

//Console.ReadLine();
#endregion

//4 tane exchange tipi vardır.
//1. Fanout Exchange: Herhangi bir filtre yapmadan kendisine bağlı olan tüm kuyruklara mesajları iletir.
//2. Direct Exchange:
//3. Topic Exchange:
//4. Header Exchange:

#region Fanout Exchange ile rabbitmq kullanımı. 
var factory = new ConnectionFactory { HostName = "localhost" };

var connection = factory.CreateConnection(); //Bağlantıyı sağladık.

var channel = connection.CreateModel(); //Yeni bir kanal oluşturduk.

//İlk parametrede bir isim veriyoruz, ikinci parametre durable fiziksel olarak kaydedilsin. Uygulamaya restart atsakta exchange kaybolmasın. 3.parametre ile de exchange tipi belirtiliyor.
channel.ExchangeDeclare("log-fanout", durable: true, type: ExchangeType.Fanout);

channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume("hello-queue", false, consumer);

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    Thread.Sleep(1000); //Görevilmek için 1 saniyelik gecikme verdik.

    Console.WriteLine("Gelen Mesaj:" + message);

    channel.BasicAck(e.DeliveryTag, false);
};

Console.ReadLine();
#endregion