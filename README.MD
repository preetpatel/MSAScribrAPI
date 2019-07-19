# Phase 2 API DevOps

For this module we will be placing our existing <i>Scribr</i> React Application and API into continuous development pipelines. This allows us to have various benefits:
* You don't need to upload code to Azure every time you make a commit
* Your code is automatically built on a virtual machine on Azure and deployed on your web app
* You can add tests that run before deploying your code to make sure that it is free of errors

But why would you want to deploy code using this method compared to just building on your laptop and uploading it to Azure?
* Your computer may have different environmental variables (Version numbers, etc..) compared to the VM on Azure which may make you assume that just because it worked on your computer, it would work when uploaded to azure. That's a wrong way of thinking as the abovementioned environment variables can cause the build to crash on Azure
* This pipeline makes sure that you do not publish broken builds to your Web App as the app is compiled and checked for compile errors before uploading to production
* It allows you to automatically detect and remove any passwords or confidential values before your website hits the production site (Not covered in this module)

Cool, now that we've covered why it is recommended to use a CI/CD environment in productions of projects, lets get started...


## Before you start
Make sure your github repo for the API looks like this. That is:

![](https://i.imgur.com/IwD0sej.png)

* Your ScribrAPI.sln file and ScribrAPI folder are on the root of the repo
    * All your context and controllers are in the ScribrAPI folder

Also, make sure you have: 
* Azure Account with Azure for Students subscription



# API on DevOps
If you've followed all previous tutorials properly, converting your project into a DevOps project should be super easy.

Get started by clicking "Create Resource" -> DevOps -> DevOps Project
![](https://i.imgur.com/hIBoRZB.png)

Once in the setup, you can start a project from fresh using the given templates (.NET, Node.js, PHP, etc..) but since we already have our coded previously and on Github, we are going to select the "Bring your own code" option
![](https://i.imgur.com/cKXoTKs.png)

Now you want to select the *Code Repository* as GitHub and go through the process of selecting your repo and branch (commonly the master branch unless you've made seperate branches)
![](https://i.imgur.com/bn3sKOp.png)
![](https://i.imgur.com/RO1h0ED.png)

Cool! Once you've done that, press next and you will get to the page that asks you to configure your framework. Since the API was coded in a .NET environment, we will choose the runtime as *.NET* and the application framework as *ASP.NET Core*
Warning! By default, the framework is selected as ASP.NET. Please be sure to choose ASP.NET Core
![](https://i.imgur.com/PSWp0oO.png)

Press next, and choose the Azure service as Windows Web App. This is becaue our project is an ASP.NET app and it does not make sense to run it on Linux as Windows is supported by default.
![](https://i.imgur.com/sZhPK9m.png)

Press next, and we're almost set up! We want to give our new devops project a name and organisation. If you dont have an organisation, create a new one.
Now under Pricing tier, press "Additional Settings" and select the F1 Free tier as shown in the illustration below.
![](https://i.imgur.com/XRsGHJX.png)

Perfect! Press next and wait for the deployment to be completed. This may take a while so stretch for a bit.
Once you're done, you'll see the page below and you want to select "Go to resource"
![](https://i.imgur.com/W40AKwS.png)

And that's pretty much it. If you've configured everything properly, you will see the follwing stages shown below where your code is compiled in the "Build" stage and then sent to dev to be released onto your web app. You can track the status of each of the tasks by clicking on the "in progress" links.
![](https://i.imgur.com/M4TZnPp.png)
![](https://i.imgur.com/TXOSRhx.png)

Once the build and release stages are complete, click on the link for your web app and hopefully your app shows up. Congratulations! At this stage, you've completed the steps to automate the build and release of your project every time you commit a change to github. No need to re-upload and re-compile... Isn't that cool!

(MARC TO ADD UNIT TESTING BIT)

# Front End on DevOps
