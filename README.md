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

* [Set-HeroAvatar](./src/HoNAvatarManagement.PowerShell/Docs/Set-HeroAvatar.md)
* [Set-HoNPath](./src/HoNAvatarManagement.PowerShell/Docs/Set-HoNPath.md)
