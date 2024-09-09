| <!--     --> | <!--                     --> |
| ------------ | ---------------------------- |
| **Author:**  | Dorian Gray II               |
| **Email:**   | doriangray_thatsme@yahoo.com |
| **Date:**    | September 8, 2024            |
| **Version:** | 1.0.0                        |

# MOM-2022-

A reconstructed Visual Studio 2017 c# source code project to allow compilation of the Assembly-CSharp.dll binary from source code.

1. A backup of the existing "C:\Program Files (x86)\Steam\steamapps\common\Master-Of-Magic\MoM_Data\Managed\Assembly-CSharp.dll" file should be made.
2. That would be replaced by the build output from this project.
3. The build output will be located:
  - .\MOM(2022)\Assembly-CSharp\bin\Debug\Assembly-CSharp.dll  - or -
  - .\MOM(2022)\Assembly-CSharp\bin\Release\Assembly-CSharp.dll
4.  The build environment will also make a copy of the .NET Module References, as part of the default build configuration.  Those can pretty much be ignored as they are just local copies of the files that already exist in the ".\MoM_Data\Managed\" subdirectory listed below.


# Build Environment

  - Target Framework: .NET Framework 4.7.1
  - Language Version: C# 7.3
  - Build Environment: Microsoft Visual Studio 2017 Version 15.9.65

  - The .NET Module References are set to the "C:\Program Files (x86)\Steam\steamapps\common\Master-Of-Magic\MoM_Data\Managed\" directory.  
    - As long as you have Master of Magic (2022) installed in the default program directory via Steam, the project should just build without having to install anything else.

# Fun Things To Try

As I think of them, I will add some interesting things to play around with.

## Default Sight Range

The default sight range used in the game can easily be changed here: [Group.cs](https://github.com/DorianGrayII/MOM-2022-/blob/ebdccdb448898b5b775c544dbc06bfa5ca7948be/MOM(2022)/Assembly-CSharp/MoM/Group.cs)

Just look for the method:

        public int GetSightRange()
        {
            return 1 + this.GetSightRangeBonus();
        }

and tweak the 1 to another value.  This should have been configurable via an XML value.  I personally like changing it to a 2 as it gives Master of Magic a more of a Civ6 feel while exploring.

## World Map Hardcoded Values

IIRC, those are located here: [FSMWorldGenerator.cs](https://github.com/DorianGrayII/MOM-2022-/blob/ebdccdb448898b5b775c544dbc06bfa5ca7948be/MOM(2022)/Assembly-CSharp/MoM/FSMWorldGenerator.cs)

Look for ```DifficultySettingsData.GetSettingAsInt("UI_WORLD_SIZE")``` and the method: 

        private IEnumerator WorldBuilder()

## How To Increase The Number of NPC Wizards?

This is not insurmountable as there are areas in the code that can handle the additional NPC Wizards without too much difficulty.  The following are a list of suggestions on where I would start...

1.  Number of NPC Wizards is tied to Wizard colors, so that will have to be addressed 1st, which is really not that hard at all.
-  The following would need to be expanded (from PlayerWizard.cs):

        public enum Color
        {
            None = 0,
            Green = 1,
            Blue = 2,
            Red = 3,
            Purple = 4,
            Yellow = 5,
            MAX = 6
        }
- Every source file that has a dependency on PlayerWizard.Color would need modification, to include:
  - WizardColors.cs
  - PopupWizardBanished.cs
  - BattleHUDInfo.cs
  - PostBattle.cs
  - PreBattle.cs

2.  I see the biggest challenge is implementing the required changes to the Wizard Stats and Wizard Diplomacy UI elements... and that is about the point where I stopped when previously considering it.  The UI code reference elements contained in the Unity asset definitions. Those details were unplublished and would require additional investigation.
