# Petrol Pump Management & Inventory ERP System

[![WPF](https://img.shields.io/badge/WPF-.NET_8-512BD4?style=for-the-badge&logo=windows&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/) [![C#](https://img.shields.io/badge/C%23-MVVM_Architecture-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/) [![PostgreSQL](https://img.shields.io/badge/PostgreSQL-EF_Core-336791?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Author](https://img.shields.io/badge/Author-Abdussatar-E50914?style=for-the-badge&logo=github&logoColor=white)](https://github.com/abdussatarkhan)

> **A comprehensive enterprise desktop ERP application for fuel stations built with WPF (.NET 8), MVVM design pattern, Entity Framework Core, and PostgreSQL — managing multi-nozzle meter readings, fuel tank inventory, credit ledger accounts, and daily shift reconciliation.**

---

## 🏛️ System Architecture

```mermaid
graph TD
    UI[WPF Views & XAML Controls] --> VM[ViewModels & MVVM Commands]
    VM --> Services[Business Services: Fuel Dispense & Inventory]
    Services --> EF[EF Core Data Access Layer]
    EF --> PG[(PostgreSQL Database)]
```

---

## 🌟 Key Features & Capabilities

- **Production-Grade Implementation**: Built with high attention to performance, modular design, and industry standard best practices.
- **Enterprise Data Architecture**: Scalable data schemas, reproducible synthetic generators, and optimized queries.
- **Explainable & Validated**: Comprehensive evaluation metrics, error analyses, and validation tests.
- **Comprehensive Tech Stack**: `C#` `WPF` `.NET 8` `MVVM` `Entity Framework Core` `PostgreSQL` `XAML`.


---

## 🚀 Quickstart & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/abdussatarkhan/petrol-pump-Farooq.git
cd petrol-pump-Farooq
```

### 2. Environment Setup
```bash
# Create and activate virtual environment
python -m venv venv
source venv/bin/activate  # On Windows: .\venv\Scripts\activate

# Install dependencies (if requirements.txt exists)
pip install -r requirements.txt
```

---

## 👨‍💻 Author & Profile

Built and maintained by **Abdussatar** ([@abdussatarkhan](https://github.com/abdussatarkhan)).  
For technical discussions, collaboration, or queries, feel free to reach out via [LinkedIn](https://www.linkedin.com/in/abdus-satar-5150813b5/) or [GitHub](https://github.com/abdussatarkhan).

---

## 📜 License

This project is licensed under the **MIT License** — see the LICENSE file for details.
