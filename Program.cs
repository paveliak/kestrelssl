using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

var sslCertificate = new X509Certificate2(Environment.GetCommandLineArgs().Last());
builder.WebHost.UseKestrel((context, options) =>
{
    options.ListenAnyIP(5000, listenOptions => 
    {
        listenOptions.UseHttps(sslCertificate);
    });
});

var app = builder.Build();

app.MapGet("/", () => Run());

app.Run();

string Run()
{
    var sb = new StringBuilder();
    sb.Append($"{sslCertificate.SubjectName.Name}\n");

    using var chain = new X509Chain();
    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
    bool result = chain.Build(sslCertificate);

    sb.AppendFormat($"Chain success={result} with length={chain.ChainElements.Count}\n");
    foreach (var item in chain.ChainElements)
    {
        sb.AppendFormat("-----\n");
        sb.AppendFormat($"ISS={item.Certificate.IssuerName.Name}\n");
        sb.AppendFormat($"SUB={item.Certificate.SubjectName.Name}\n");
    }

    return sb.ToString();
}