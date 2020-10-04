# HoNAvatarManager

Hero avatar manager for Heroes of Newerth.

### How to install

1. Install [PowerShell Core](https://github.com/PowerShell/PowerShell/releases/latest)
2. Run PowerShell Core as Administrator
3. Run `Install-Module HoNAvatarManager`

### How to run

1. Run `Import-Module HoNAvatarManager`
2. To make the completion process easier, run `Set-PSReadlineKeyHandler -Key Tab -Function MenuComplete`

## Cmdlets list

* [Set-HeroAvatar](./src/HoNAvatarManagement.PowerShell/Docs/Set-HeroAvatar.md)
* [Set-HoNPath](./src/HoNAvatarManagement.PowerShell/Docs/Set-HoNPath.md)
