# 📚 TaskFlow API – Projet académique

**TaskFlow** est une API REST développée en .NET 8 dans le cadre d’un projet académique.
Elle permet de gérer des utilisateurs, des projets et des tâches, avec une authentification sécurisée via JWT.

---

## 🌟 Objectifs pédagogiques

Ce projet démontre la mise en place de :

- Entity Framework Core (avec SQL Server)
- Authentification JWT
- Architecture propre (DTOs, Middleware, Injection de dépendance)
- Gestion d’erreurs centralisée
- Documentation Swagger

---

## ⚙️ Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) installé
- Docker installé et démarré
- (Facultatif) Un client SQL type Azure Data Studio ou DBeaver

---

## 🐳 Lancer SQL Server avec Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your_password123" \
  -p 1433:1433 --name taskflow-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

---

## 🔧 Configurer la connexion

Dans `TaskFlow.Api/appsettings.json` :

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=TaskFlowDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
}
```

---

## 🧱 Appliquer les migrations EF Core

```bash
dotnet ef database update \
  --project TaskFlow.DAL \
  --startup-project TaskFlow.Api
```

---

## 🚀 Lancer l’API

```bash
dotnet run --project TaskFlow.Api
```

---

## 📘 Accéder à la documentation Swagger

> La doc Swagger est générée automatiquement :

📍 [http://localhost:5000/swagger](http://localhost:5000/swagger) (ou `https://localhost:5001/swagger`)

---

## 🔒 Authentification

- `POST /api/users/register` : inscription
- `POST /api/users/login` : obtention d’un token JWT

Ensuite, ajoutez ce token dans Swagger via le bouton `Authorize 🔒`.

---

## 📦 Endpoints REST principaux

| Ressource | Méthode | Route             | Authentification requise |
| --------- | ------- | ----------------- | ------------------------ |
| Projects  | GET     | `/api/projects`   | ✅ Oui                   |
| Projects  | POST    | `/api/projects`   | ✅ Oui                   |
| Tasks     | GET     | `/api/tasks`      | ✅ Oui                   |
| Tasks     | PUT     | `/api/tasks/{id}` | ✅ Oui                   |

> Toutes les routes sont sécurisées (JWT obligatoire) sauf `/register` et `/login`.

---

## 🧪 Seed de la base de données via Swagger

Une route est disponible pour initialiser des données de démonstration :

- `POST /api/seed` (accessible sans authentification)

Elle crée automatiquement :

- un utilisateur `demo@taskflow.com` / `demo123`
- un projet de démo
- deux tâches liées

Vous pouvez ensuite utiliser cet utilisateur pour tester l’API.

---

## 🔪 Tests manuels

Vous pouvez tester l’API via :

- Swagger UI
- Postman
- `curl` (voir section seeding)

---

## 🪮 Nettoyer le conteneur Docker (facultatif)

```bash
docker stop taskflow-sql
docker rm taskflow-sql
```

---

## ✍️ Auteur

Projet réalisé par **Maxime Arnould** et **Josue-remi Biyoghe-obiang** dans le cadre du cours .NET / SUPINFO 2025.
