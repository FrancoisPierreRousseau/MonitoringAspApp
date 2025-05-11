# Windows Event Logger Terminal Application

## 📦 Introduction

Ce projet démontre comment surveiller et afficher en temps réel les événements d'une application .NET sans passer exclusivement par l'Observateur d'événements Windows. Le but est d'écouter les événements et de les afficher directement dans le terminal selon des filtres configurables.

---

## ✅ Prérequis
- .NET 6 ou supérieur
- Accès administrateur pour la création de sources EventLog
- PowerShell (pour les tests rapides)

---

## ⚙️ Configuration du fichier `config.json`
Le fichier `config.json` permet de configurer les sources et les filtres pour surveiller les logs. Voici un exemple :

```json
{
    "log_type": "Application",
    "sources": ["MyApiSource"],
    "filters": [
        ".*Error.*",
        ".*Warning.*",
        ".*Test.*"
    ]
}
```

- `log_type` : Type de journal Windows (Application, System, Security).
- `sources` : Liste des sources à surveiller.
- `filters` : Liste de filtres Regex pour le contenu des messages.

---

## 🚀 Lancer le projet
1. Ouvrir un terminal en **mode administrateur**.

2. Exécuter le projet :

```bash
cd MontoringAspApp
 dotnet run -- config.json
```

---

## Lancer le projet en mode debug

Placer le code suivant au début du projet
```csharp
Debugger.Launch();
```

Cela attachera le debugger lors de l'éxécution du processus.  

Lancez le projet:

```bash
cd MontoringAspApp
 dotnet run -- config.json
```


---

## 🛠️ Exemple d'API ASP.NET loguant dans l'EventLog

### Créer la source EventLog en utilisant powershell

```powershell
New-EventLog -LogName Application -Source "MyApiSource"
```

### Program.cs
---
Dans une API ASP.NET, configurez le `Program.cs` comme suit :

```csharp
var sourceName = "MyApiSource";
var logName = "Application";

if (!EventLog.SourceExists(sourceName))
{
    EventLog.CreateEventSource(sourceName, logName);
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddEventLog(settings =>
{
    settings.SourceName = sourceName;
    settings.LogName = logName;
});
```

---
  
### ✅ ProduitController.cs
  
Dans le contrôleur `ProduitController.cs`, on peut logger un événement comme suit :

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProduitController : ODataController
{
    private readonly ILogger<ProduitController> _logger;

    public ProduitController(ILogger<ProduitController> logger)
    {
        _logger = logger;
    }

    [HttpGet("odata/[controller]")]
    public IActionResult Get(ODataQueryOptions<Produit> options)
    {
        _logger.LogError("This is a test error log from ProduitController!");

        var produits = new[] { new Produit { Id = 1, Name = "Produit A" } }.AsQueryable();
        var result = options.ApplyTo(produits);

        return Ok(result);
    }
}

public class Produit
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

## 🔥 Tester l'API
1. Lancer l'API :

```bash
dotnet run
```

2. Générer un log d'erreur :

Accéder à : `http://localhost:5000/api/produit/test-log`

3. Vérifier le terminal de l'application EventLoggerTerminalApp pour les logs générés.

---

## ✅ Conclusion
Ce projet permet de centraliser les logs d'applications ASP.NET Core en temps réel dans un terminal. Il est particulièrement utile pour les environnements de développement et de test où l'Observateur d'événements n'est pas suffisant ou ergonomique.
