# MA2Export2File ![.NET Core](https://github.com/hossimo/MA2Export2File/workflows/.NET%20Core/badge.svg)
 Terminal application that takes an MA2 Image Export XML file and extracts the included images. Should run on Windows Mac or Linux with the **[.NET](https://dotnet.microsoft.com/download)** framework.

 ## [Download](https://github.com/hossimo/MA2Export2File/releases)

 ![IMAGE](https://github.com/hossimo/MA2Export2File/blob/master/Images/example.png)
 


### Issues:
* There isnt very much validation code in this at the moment so it may.. or may not work.
* Make sure you have **[.NET](https://dotnet.microsoft.com/download)**
#### MacOS:
* Due to the way Github actions zips files the executable for macOS is not ... executable. to fix this run `chmod u+x MA2ImageExport2File` on the unzipped content.
* Newer versions of macOS will complain because the code isnt codesigned, I don't make money from this so I likly wont pay for the cerfificate. to get around that Right click on MA2ImageExport2File from finder, Click Open then in the big scary dialog click Open again. This will allow you to run the application from the terminal on subsiquent runs.

OSX build currently has an issue where it's not executable
