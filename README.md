# Tovia – Desktop Task Manager with Microsoft & Google Integration (WPF)

Tovia is a Windows desktop task manager designed to unify your daily tasks across platforms.
It integrates directly with Microsoft Outlook Tasks and is being expanded to support Google Tasks, letting you manage everything from a single, distraction-free WPF app.

Stay focused, keep your tasks visible, and sync seamlessly — without living in a browser.

---
## Screenshots / Demo
<table>
  <tr>
    <td>
      <img width="420"  alt="image" src="https://github.com/user-attachments/assets/648c9ac8-3944-43c1-9f65-ec0f8fd46343" />
      <p align="center"><i>Creating a task in Tovia</i></p>
    </td>
    <td valign="top">
      <img src="https://github.com/user-attachments/assets/ee8c4c0c-65e5-4825-b539-52125e3277a4" alt="Sync Task Creation" width="420" style="display:block; margin-bottom:10px;" />
      <p align="center"><i>Two-way sync: tasks appear instantly in Outlook Tasks.</i></p>
      <img src="https://github.com/user-attachments/assets/c11e4a81-0058-4450-9675-c28061b67860" alt="Sync Event Creation" width="420" style="display:block;" />
      <p align="center"><i>Create Outlook Calendar events directly from Tovia tasks.</i></p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/2624023a-8835-4f6a-baa0-b594a710ef46" alt="Overlay Panel" width="420"/>
      <p align="center"><i>Overlay Window in sync with Outlook.</i></p>
    </td>
  </tr>
</table>

---
## Current Integrations

| Provider                | Status         |
| ----------------------- | -------------- |
| Microsoft Outlook Tasks | ✅ Implemented  |
| Microsoft Calendar      | ✅ Implemented  |
| Google Tasks            | ✅ Implemented  |
| Google Calendar         | 🔜 Planned     |

---

## Features

- 🔒 **Microsoft Account Integration** – Securely sign in and connect to your Outlook account.  
- 🔄 **Two-way Task Sync** – Tasks added in Tovia automatically sync with Outlook Tasks.  
- 📅 **Calendar Event Creation** – Convert tasks into calendar events with reminders.  
- 🖥️ **Overlay View** – Keep tasks visible on-screen without interrupting your workflow.  
- 🧪 **Testing Workflow** – Unit-tested architecture for reliability and maintainability.  
- 🛠️ **Clean Architecture** – Built with MVVM, Dependency Injection, WPF, XAML, and Material Design for scalable and maintainable code.

---

## Technology Stack

- **Language:** C# / .NET 8  
- **UI Framework:** WPF, XAML, Material Design  
- **Architecture:** MVVM, Dependency Injection  
- **API:** Microsoft Graph API (for Outlook integration)  
- **Version Control:** Git / GitHub
