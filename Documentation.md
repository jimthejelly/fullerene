# Documentation Guidelines

### Location
Documentation is located in *./Docs/html*. When downloaded, documentation can be viewed by opening the *index.html* file in the aforementioned folder.

### Generation
Documentation is generated with [Doxygen](https://www.doxygen.nl/). In order to document the codebase, download and open the Doxywizard client. First, select the home folder of the project as the Working Directory. Change the Project Name to Fullerene and add a synopsis or version number if you feel the need. Then, select the *./Assets/Scripts* folder as the Source Code Directory and select Scan Recursively. Next, input the *./Docs* folder as the Destination Directory. Finally, navigate to the Output subtab of the Wizard tab and deselect LaTeX as an output format. From here, select the Run tab and click Run Doxygen. If all these steps have been followed properly, you should see an updated set of files in the *./Docs/html* folder.