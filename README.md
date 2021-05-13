# HoNAvatarManager

Hero avatar manager for Heroes of Newerth.

### How to install

1. Install [PowerShell Core](https://github.com/PowerShell/PowerShell/releases/latest)
2. Run PowerShell Core as Administrator
3. Run `Install-Module HoNAvatarManager`

### How to update

1. Run PowerShell Core as Administrator
2. Run `Install-Module HoNAvatarManager -Force`

### How to run

1. Run PowerShell Core as Administrator
2. Run `Import-Module HoNAvatarManager`
3. To make the completion process easier, run `Set-PSReadlineKeyHandler -Key Tab -Function MenuComplete`

## Cmdlets list

* [Remove-HeroAvatar](./src/HoNAvatarManagement.PowerShell/Docs/Remove-HeroAvatar.md)
* [Set-HeroAvatar](./src/HoNAvatarManagement.PowerShell/Docs/Set-HeroAvatar.md)
* [Set-HoNPath](./src/HoNAvatarManagement.PowerShell/Docs/Set-HoNPath.md)

### Как установить

1. Установите [PowerShell Core](https://github.com/PowerShell/PowerShell/releases/latest)
  * Если у вас Windows
    * Если у вас x64 версия Windows, установить **PowerShell-win-x64.msi**
    * Если у вас x32 версия Windows, установить **PowerShell-win-x86.msi**
  * Если у вас Linux
    * Если у вас Ubuntu, установить **powershell.ubuntu.amd64.deb**
    * Если у вас Debian, установить **powershell.debian.amd64.deb** 
    * Если у вас другая версия Linux, установить **powershell.linux.x64.deb** 
2. Запустите PowerShell Core как Администратор
3. Введите `Install-Module HoNAvatarManager` в PowerShell Core и нажмите Enter

### Как обновить

1. Запустите PowerShell Core как Администратор
2. Введите `Install-Module HoNAvatarManager -Force` в PowerShell Core и нажмите Enter

### Как использовать

1. Запустите PowerShell Core как Администратор
2. Введите `Import-Module HoNAvatarManager` в PowerShell Core и нажмите Enter
3. Чтобы упростить процесс использования, введите `Set-PSReadlineKeyHandler -Key Tab -Function MenuComplete` и нажмите Enter
4. Для того, чтобы установить аватар, введите `Set-HeroAvatar -Hero`, нажмите Tab, выберите героя, введите `-Avatar`, выберите аватар и нажмите Enter
