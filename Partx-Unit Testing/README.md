# Unit Testing
For those of you lucky souls who have 
## Why do we Unit Test?

## When to Unit Test
One of the biggest debates surrounding software testing is whether or not you should write your test *before* or *after* the actual code. Many people say you should write your tests for a class/module before you implement it. This is known as Test Driven Development (TDD). Whether or not this is actually the case in practice is another story...

The main arguments for TDD are that it provides clarity and a motivation for simplicity. By writing the tests for a unit before you implement it, you get to think about the requirements of your code so you get a better idea of how to write it because you understand what it should do. Additionally, you shouldn't theoretically add any unnecessary functionality because you're only writing the code so that it passes the tests - ergo, simplicity!

Sometimes requirements are liable to change and you can get a better understanding of the problem by going ahead and solving it. Hence, a lot of developers prefer to write tests after the code is written. The problem with this is that it can be hard to stay "on topic" and it's easy to write redundant code. 
However, as new programmers / being new to a certain language or framework, it's quite infeasible to write our tests beforehand because we might not even understand how the code is going to work. This is precisely why we've waited until a later module of MSA phase 2 to show you how to do unit tests for an ASP.NET core web API.

So without further ado, let's get underway!

# Writing Unit Tests for ScribrAPI
### Creating the Test Project
Have the ScribrAPI solution open in Visual Studio. Start by adding an MSTest project to it.

File -> New -> Project

Scroll down to MSTest Test Project (.Net Core), select it, then click "Next".

![](https://github.com/LindaBot/ScribrAPI/blob/MB/UnitTests/Partx-Unit%20Testing/images/CreateMSTestProject.png)

Give the project a name. Make sure to select "Add to solution" as shown in the image below, then click "Create".

![](https://github.com/LindaBot/ScribrAPI/blob/MB/UnitTests/Partx-Unit%20Testing/images/CreateMSTestProject2.png)

### Setting up the Test Project

Start by adding a reference from the newly created test project to the API project. Right click on the unit test project in the solution explorer:
![](https://github.com/LindaBot/ScribrAPI/blob/MB/UnitTests/Partx-Unit%20Testing/images/ProjectInSolExplorer.png)

Then go Add -> Reference...
![](https://github.com/LindaBot/ScribrAPI/blob/MB/UnitTests/Partx-Unit%20Testing/images/AddReference.png)

In the Reference Manager window select the API project as show in the image below. Then select "Ok".
![](https://github.com/LindaBot/ScribrAPI/blob/MB/UnitTests/Partx-Unit%20Testing/images/AddMainProjectReference.png)

Now we're going to add a couple of packages that will let us use a mock database (More on this in a bit).
Right click on the project solution and select "Manage NuGet Packages for solution"
Add to the solution:
* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.InMemory

## Let the Testing begin
Now we can start testing.
* Right click on the test project
* select (add -> New Item)
* create a new .cs file. Let's call it UnitTest

The code we're interested in testing is the code inside our API controllers, as this is where the actual logic is done for serving data.

Copy the following snippet to the file. It has all the imports we'll need. The attribute (a.k.a annotation) `[TestClass]` placed before the class tells MSTest that this class has methods it can run as tests.

```csharp
using ScribrAPI.Controllers;
using ScribrAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace UnitTestScribrAPI
{
    [TestClass]
    public class VideosControllerUnitTests
    {
      // Insert code here
    }
}
```
We will begin by defining all the class variables. Then we define what happens before a test is run and after a test is run. We want to intialise the mock database before a test runs and then we want to clear the mock database after a test runs. Making sure that all tests remain independent. By mocking the database we greatly improve the speed at which the tests can run as they do not have to stand up a real database.
```csharp

```
Now we can begin writing the test methods to test the API. I suggest that you start by writing tests for all the CRUD methods.
```csharp

```
```csharp
```