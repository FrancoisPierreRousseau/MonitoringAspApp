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


Réfléchir à un début de systéme pour récupérer des logs stocké chez des clients (cela peut être intéréssant de les considérer).
car c'est des données produites en temps réél. 
En utilisant le partionnement déclaratif on pourrait ordonnée les données inséré par (type de log / dates).Pour récupérer des logs à distance, ont utilise pas mal le protocole RFC qui est spécifiquement conçus pour cela.
Ce sont des log définit en format standardisé pour les messages Syslog.


## Considération

Dans le cadre du projet, il est pertinent de considérer l'envoi des logs vers une base de données au lieu de simplement les afficher dans le terminal. Cette approche présente plusieurs avantages :

1. **Centralisation des données :** En stockant les logs dans une base de données, il devient possible de centraliser les informations provenant de multiples sources d'événements. Cela permet d'avoir une vision consolidée des logs pour l'ensemble du système.

2. **Requêtes avancées et analyses :** Les bases de données permettent d'exécuter des requêtes complexes pour filtrer, agréger ou analyser les données de logs. Cela peut être essentiel pour des rapports ou des analyses post-mortem.

3. **Persistance des données :** Contrairement à un affichage temporaire dans le terminal, les logs stockés en base de données sont persistants et peuvent être conservés à long terme pour des audits ou des analyses historiques.

4. **Scalabilité :** Une base de données bien conçue peut gérer un volume de logs conséquent et croissant, ce qui est particulièrement important pour des applications critiques ou à forte volumétrie.

### Partitionnement déclaratif

Pour gérer efficacement le volume de données, il peut être pertinent de mettre en place un partitionnement déclaratif au niveau des tables de logs. Ce mécanisme consiste à diviser physiquement les données en segments logiques basés sur des critères définis (par exemple, par date ou par type de log).

#### **Avantages du partitionnement déclaratif :**

* **Optimisation des performances :** Les requêtes ciblant des plages de données spécifiques accèdent uniquement aux partitions concernées, réduisant ainsi le temps de traitement.
* **Gestion des données historiques :** Les partitions plus anciennes peuvent être archivées ou supprimées plus facilement sans affecter les données récentes.
* **Scalabilité accrue :** Les tables partitionnées peuvent être réparties sur plusieurs disques ou serveurs, améliorant la capacité de traitement des logs.

Pour plus de détails sur la mise en œuvre du partitionnement déclaratif, vous pouvez consulter la documentation suivante : [Partitionnement Déclaratif](https://github.com/FrancoisPierreRousseau/database/blob/main/optimisation/Partitionnement%20D%C3%A9claratif.md).


## Cas d'utilisation étendus pour le Windows Event Logger Terminal Application

1. **Journalisation des modifications sur des ressources spécifiques :**

   * Objectif : Suivre les modifications effectuées par les utilisateurs (consultation, mise à jour, création, suppression) sur des ressources critiques telles que les documents, les comptes ou les configurations système.
   * Scénario : Lorsqu'un utilisateur accède à une ressource sensible ou effectue une modification critique, un événement est généré et consigné dans le journal avec des informations détaillées : utilisateur, type de modification, timestamp, etc.
   * Exemple : Un utilisateur accède à une fiche client et met à jour l'adresse. Un événement "Modification - Fiche Client" est enregistré avec le nouvel état de la fiche.

2. **Suivi des temps d'activité et d'inactivité des utilisateurs :**

   * Objectif : Identifier les périodes d'activité et d'inactivité des utilisateurs à des fins de suivi de la productivité ou pour des analyses de sécurité.
   * Scénario : Lorsqu'un utilisateur se connecte, un événement "Connexion" est enregistré. Lorsqu'il se déconnecte, un événement "Déconnexion" est consigné. La durée d'activité est calculée en temps réel.
   * Exemple : Un utilisateur se connecte à 08h00 et se déconnecte à 12h00. Un rapport peut alors être généré pour indiquer la durée totale de connexion.

3. **Processus d'alerting en cas de connexion à des heures non autorisées :**

   * Objectif : Détecter des comportements suspects ou des tentatives d'accès non autorisées en dehors des heures normales d'activité.
   * Scénario : Si un utilisateur se connecte entre 00h00 et 05h00 alors que les heures normales d'activité sont de 08h00 à 18h00, un événement "Connexion non autorisée" est généré.
   * Exemple : Un administrateur système se connecte à 02h30 du matin alors qu'aucune maintenance planifiée n'est prévue. Un événement d'alerte est généré et notifié à l'équipe de sécurité.

4. **Surveillance des tentatives d'accès non autorisées :**

   * Objectif : Détecter des tentatives d'accès à des ressources protégées par des utilisateurs non autorisés ou des comptes compromis.
   * Scénario : Chaque fois qu'un utilisateur tente d'accéder à une ressource sans les autorisations appropriées, un événement "Accès refusé" est généré.
   * Exemple : Un employé du service commercial tente d'accéder à des informations financières réservées à la comptabilité.

5. **Centralisation des logs multi-applications :**

   * Objectif : Agréger les logs de plusieurs applications dans un terminal unique pour une surveillance centralisée.
   * Scénario : Les logs d'une application API, d'un service backend et d'un service de base de données sont tous envoyés à l'Event Logger Terminal Application, permettant une visualisation unifiée des événements.
   * Exemple : Lorsqu'un incident critique se produit, les événements provenant de toutes les applications concernées sont regroupés pour une analyse complète.

6. **Gestion des logs historiques pour audit et reporting :**

   * Objectif : Archiver les logs critiques pour des audits ultérieurs ou pour générer des rapports mensuels ou annuels.
   * Scénario : Les logs concernant les accès aux informations confidentielles sont sauvegardés tous les mois dans une base de données centralisée.
   * Exemple : Lors d'un audit, l'équipe de sécurité extrait les logs des six derniers mois pour vérifier les accès à des dossiers clients sensibles.

7. **Partage des logs avec des services de monitoring externes :**

   * Objectif : Transmettre certains logs à des services d'analyse ou de SIEM (Security Information and Event Management) pour des analyses approfondies ou des alertes automatisées.
   * Scénario : Les logs d'erreur critique sont envoyés à un service comme Splunk ou ELK pour une analyse en temps réel et une visualisation graphique.
   * Exemple : Lorsqu'une erreur critique est détectée, un événement est envoyé à un service d'alerte qui déclenche une notification Slack à l'équipe de support.

Ces cas d'utilisation couvrent à la fois des scénarios d'analyse de logs en temps réel, des mécanismes d'alerte, des audits d'activité et des intégrations externes, offrant ainsi une vision complète des possibilités du système.

