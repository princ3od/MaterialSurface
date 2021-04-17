# MaterialSurface - UI Library for Winform C#
This library brings Google's Material Design components to Winform C#.
## Library Components
- **Button** (Contained Button/Outlined Button/Text Button)
- **Card** (2 types)
- **ComboBox** (3 types)
- **CheckBox**
- **Dialog** (support DarkTheme)
- **RadioButton**
- **ChoiceChip** (2 types)
- **ProgressBar** (2 types)
- **TextField** (3 types)
- **Snackbar** (support DarkTheme)
## Installation
### 1. Adding library to your project
  You can add library through the Package Manager Console ([NuGet Package](https://www.nuget.org/packages/MaterialSkin/)).  
  Click **View** > **Others Windows** > **Package Manager Console** and run this command:
 ```
 PM > Install-Package MaterialSurface -Version 1.0.0
 ```
 Or you can clone this project from Github, compile it yourself and add output as a reference.
 ### 2. Adding components to your Toolbox
  Once you have installed the package, all components should be stayed in tab "MaterialSurface" of your IDE's Toolbox.  
  Otherwise, you can manually add by right clicking Toolbox > Choose Items... and browse to the MaterialSurface.dll file in folder "..\\\packages\MaterialSurface.1.0.0\lib\".
 ### 3. Using library
  Just use it as normal Winform controls except Dialog and Snackbar.  
  - Dialog is a replacement of MessageBox, so use it like MessageBox.  
  - Using Snackbar with method MakeSnackbar (static) or Make (for object).
  *I strongly recommend cloning project and taking a look at MaterialSurfaceExample which has Playground to explore all components.*
## Contact
- Facebook: https://www.facebook.com/princ3od/
- LinkedIn: https://www.linkedin.com/in/princ3od/
- Email: princ3od@gmail.com
## Note
Don't forget to check [MaterialSkin](https://github.com/IgnaceMaes/MaterialSkin) which is a similar project but having missing components from this project such as Drawer, TabControl, Tooltip...
## Preview Images
//empty
