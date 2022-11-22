using System.Net.Http.Json;
using LocalNetAppChat.Domain;


// using (WebClient client = new WebClient())
// {
//     var result = client.DownloadString("http://localhost:5000/receive");
//     Console.WriteLine(result);
// }


using (HttpClient client = new HttpClient())
{
    Message message = new Message(
        Guid.NewGuid().ToString(),
        "Lukas",
        "Hi ich bin Lukas",
        Array.Empty<string>(),
        true,
        "Message"
    );

    var result = await client.PostAsJsonAsync("http://localhost:5000/send", message);
    
    Console.WriteLine(result);
}

