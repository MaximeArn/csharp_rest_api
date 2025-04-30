This is an academic project to validate the knowledges on c# dotnet

# 🛠️ Setup de la base de données (SQL Server via Docker)

Ce projet utilise SQL Server comme SGBD.
Voici les étapes à suivre pour faire tourner la base de données localement sur Mac (ou autre OS) via Docker.

---

## 1. Prérequis

- Docker installé et en cours d'exécution
- .NET SDK installé (v6 ou v8)

---

## 2. Lancer SQL Server avec Docker

Exécutez cette commande dans votre terminal :

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your_password123" \
  -p 1433:1433 --name taskflow-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### Paramètres personnalisables :

- `SA_PASSWORD` : mot de passe de l'utilisateur `sa` (doit respecter les règles de sécurité)
- `taskflow-sql` : nom du container Docker

---

## 3. Configuration de la connexion (appsettings.json)

Vérifiez que le fichier `TaskFlow.Api/appsettings.json` contient :

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=TaskFlowDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
}
```

---

## 4. Créer la base via Entity Framework Core

```bash
dotnet ef database update \
  --project TaskFlow.DAL \
  --startup-project TaskFlow.Api
```

> Cela applique les migrations et crée la base `TaskFlowDb` dans SQL Server.
