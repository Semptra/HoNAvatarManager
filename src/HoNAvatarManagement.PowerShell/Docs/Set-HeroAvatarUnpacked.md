---
external help file: HoNAvatarManager.PowerShell.dll-Help.xml
Module Name: HoNAvatarManager
online version:
schema: 2.0.0
---

# Set-HeroAvatarUnpacked

## SYNOPSIS
Sets the default hero avatar to the selected one. Avatar resources are put directly into the /game/heroes directory.

## SYNTAX

```
Set-HeroAvatarUnpacked -Hero <String> -Avatar <String> [<CommonParameters>]
```

## DESCRIPTION
Sets the default hero avatar to the selected one. Avatar resources are put directly into the /game/heroes directory.

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-HeroAvatarUnpacked -Hero "Behemoth" -Avatar "alt5"
```

### Example 2
```powershell
PS C:\> Set-HeroAvatarUnpacked -Hero "Devourer" -Avatar "alt9"
```

## PARAMETERS

### -Avatar
Avatar name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Hero
Hero name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
