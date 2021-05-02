using System.Collections.Generic;
using System.IO;
using HoNAvatarManager.Core.Exceptions;

namespace HoNAvatarManager.Core.Helpers
{
    internal static class ThrowHelper
    {
        public static DirectoryNotFoundException DirectoryNotFoundException(string message)
        {
            return new DirectoryNotFoundException(AppendSubmitIssue(message));
        }

        public static AnimationNotFoundException AnimationNotFoundException(string message, string animation)
        {
            return new AnimationNotFoundException(AppendSubmitIssue(message), animation);
        }

        public static AvatarNotFoundException AvatarNotFoundException(string message, string avatar)
        {
            return new AvatarNotFoundException(AppendSubmitIssue(message), avatar);
        }

        public static FileNotFoundException FileNotFoundException(string message)
        {
            return new FileNotFoundException(AppendSubmitIssue(message));
        }

        public static FileNotFoundException FileNotFoundException(string message, string fileName)
        {
            return new FileNotFoundException(AppendSubmitIssue(message), fileName);
        }

        public static KeyNotFoundException KeyNotFoundException(string message)
        {
            return new KeyNotFoundException(AppendSubmitIssue(message));
        }

        private static string AppendSubmitIssue(string message)
        {
            return message + " Submit an issue: https://github.com/Semptra/HoNAvatarManager/issues/new";
        }
    }
}
