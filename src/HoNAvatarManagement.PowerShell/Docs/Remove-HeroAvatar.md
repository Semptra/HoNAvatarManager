---
external help file: HoNAvatarManager.PowerShell.dll-Help.xml
Module Name: HoNAvatarManager
online version:
schema: 2.0.0
---

# Remove-HeroAvatar

## SYNOPSIS
Removes currently selected hero avatar.

## SYNTAX

### Single
```
Remove-HeroAvatar -Hero <String> [<CommonParameters>]
```

### All
```
Remove-HeroAvatar [-All] [<CommonParameters>]
```

## DESCRIPTION
Removes currently selected hero avatar. This command removes `resources` hero file from the game directory (if exists)
and effectively sets the default avatar back.

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-HeroAvatar -Hero "Aluna"
```

## PARAMETERS

### -All
Remove all currently selected hero avatars.

```yaml
Type: SwitchParameter
Parameter Sets: All
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Hero
Hero for which to remove the currently selected avatar.

```yaml
Type: String
Parameter Sets: Single
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
