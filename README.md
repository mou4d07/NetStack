NetStack
NetStack is a tool for designing infrastructure topologies, testing equipment availability via heartbeat checks, and visualizing devices on an interactive map of the complex.

Tech Stack
Frontend: Next.js (React)

Backend: ASP.NET Core API

Database: SQL Server / PostgreSQL (depending on your configuration)

Prerequisites
Node.js (v18 or later)

.NET SDK (v6 or later)

SQL Server or PostgreSQL instance

Getting Started
1. Clone the repository
bash
git clone https://github.com/your-repo/netstack.git
cd netstack
2. Backend setup (ASP.NET Core)
bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
The API will run on https://localhost:5001 by default.

3. Frontend setup (Next.js)
Open a new terminal:

bash
cd frontend
npm install
npm run dev
The Next.js app will run on http://localhost:3000.

4. Configure environment variables
Create a .env.local file in the frontend folder:

text
NEXT_PUBLIC_API_URL=https://localhost:5001/api
For the backend, create an appsettings.json with your database connection string.

Usage
Open http://localhost:3000

Create your infrastructure topology by adding equipment

Run heartbeat checks to verify devices are up

View all equipment on the interactive map

Project structure
text
netstack/
├── backend/          # ASP.NET Core API
│   ├── Controllers/
│   ├── Models/
│   └── Data/
└── frontend/         # Next.js application
    ├── app/
    ├── components/
    └── public/
