# GScore Clean Architecture Template (dotnet new)

This repository is a **.NET project template**. After installing it, you can generate a new solution by providing a new name (it will replace `GScore` everywhere).

---

## 0) Folder name requirement (IMPORTANT)

Your template folder must be:

- `.template.config/`

NOT `.tempolate.config/`

`dotnet new` only recognizes `.template.config`.

---

## 1) Prerequisites

Check .NET SDK:

```bash
dotnet --version
````

---

## 2) Prepare the template repo (one-time)

Make sure these exist:

* `.template.config/template.json`
* Your solution file name contains the source token, recommended:

  * `GScore.slnx`

> The template replaces the token: `GScore`

---

## 3) Install the template (one-time on your machine)

From the **repo root** (where `.template.config` is located):

```bash
dotnet new install .
```

Verify it appears:

* Windows:

  ```bash
  dotnet new list | findstr net-template
  ```
* macOS/Linux:

  ```bash
  dotnet new list | grep net-template
  ```

---

## 4) Create a new project from the template

Example: create a new service named `ServiceChat` into folder `ServiceChat`:

```bash
dotnet new net-template -n ServiceChat -o ServiceChat
```

Then:

```bash
cd ServiceChat
dotnet restore
dotnet build
```

---

## 5) Uninstall the template (optional)

```bash
dotnet new uninstall .
```

```