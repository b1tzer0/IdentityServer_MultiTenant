// See https://aka.ms/new-console-template for more information
using Duende.IdentityServer.Models;

using IdentityModel;

Console.Write("Enter your client secret: ");
string clientSecret = Console.ReadLine().ToSha256();

var sec = new Secret(clientSecret);
Console.WriteLine($"Your encoded client secret is: {sec.Value}");

Console.WriteLine("Press any key to exit.");
Console.ReadKey();
