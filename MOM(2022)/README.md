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
  - The .NET Module References are set to the "C:\Program Files (x86)\Steam\steamapps\common\Master-Of-Magic\MoM_Data\Managed\" directory.  
    - As long as you have Master of Magic (2022) installed in the default program directory via Steam, the project should just build without having to install anything else.
