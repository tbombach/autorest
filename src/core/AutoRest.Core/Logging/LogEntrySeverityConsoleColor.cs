﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace AutoRest.Core.Logging
{
    /// <summary>
    /// Severity of the log message.
    /// </summary>
    public static class LogEntrySeverityConsoleColor
    {
        private static IDictionary<LogEntrySeverity, ConsoleColor> _dict = new Dictionary<LogEntrySeverity, ConsoleColor>
        {
            { LogEntrySeverity.Fatal, ConsoleColor.Red },
            { LogEntrySeverity.Error, ConsoleColor.Red },
            { LogEntrySeverity.Warning, ConsoleColor.Yellow },
            { LogEntrySeverity.Info, ConsoleColor.White },
        };

        /// <summary>
        /// Get the console color associated with the severity of the message
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        public static ConsoleColor GetColorForSeverity(this LogEntrySeverity severity)
        {
            ConsoleColor color;
            if (!_dict.TryGetValue(severity, out color))
            {
                throw new ArgumentException(string.Format("No color defined for severity {0}", severity));
            }
            return color;
        }
    }
}