# MaterialSurface
This library brings Google's Material Design components to Winform C#.  
[![INTRO VIDEO](https://img.youtube.com/vi/uaxmJ60yjFs/0.jpg)](https://www.youtube.com/watch?v=uaxmJ60yjFs)
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
### [Video (Vietnamese)](https://youtu.be/AocfcB6sFGc)
### 1. Adding library to your project
  You can add library through the Package Manager Console ([MaterialSurface NuGet Package](https://www.nuget.org/packages/MaterialSurface/)).  
  Click **View** > **Others Windows** > **Package Manager Console** and run this command:
 ```
 PM > Install-Package MaterialSurface -Version 1.1.0
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
## Author
- [Trong Duong Binh](https://github.com/princ3od)
## Note
- Check out [MaterialSkin](https://github.com/IgnaceMaes/MaterialSkin) which is a similar project but conatins missing components from this project like Drawer, TabControl, Tooltip...
- Give this project a star if you found it helpful.
## Preview Images
*Home tab*
![Home Tab](https://i.imgur.com/hI5L8Ai.png)
*Button tab*
![Button Tab](https://i.imgur.com/sjx0cSk.png)
*TextField tab*
![TextField Tab](https://i.imgur.com/hjpuSRf.png)
*Toggles tab*
![Toggles Tab](https://i.imgur.com/5Q5PSih.png)
*ProgressBar tab*
![ProgressBar Tab](https://i.imgur.com/71BF8OR.png)
*Snackbar*
![Fullwidth Snackbar](https://i.imgur.com/yZIaYDx.png)
*Dialog*
![Dialog](https://i.imgur.com/yRoiO4v.png)
