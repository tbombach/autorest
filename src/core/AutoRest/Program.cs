﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoRest.Core;
using AutoRest.Core.Logging;
using AutoRest.Properties;

namespace AutoRest
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            int exitCode = (int)ExitCode.Error;

            try
            {
                Settings settings = null;
                try
                {
                    settings = Settings.Create(args);
                    if (settings.ShowHelp && IsShowMarkdownHelpIncluded(args))
                    {
                        Console.WriteLine(HelpGenerator.Generate(Resources.HelpMarkdownTemplate, settings));
                    }
                    else if (settings.ShowHelp)
                    {
                        Console.WriteLine(HelpGenerator.Generate(Resources.HelpTextTemplate, settings));
                    }
                    else
                    {
                        Core.AutoRest.Generate(settings);
                    }
                }
                catch (CodeGenerationException)
                {
                    // Do not add the CodeGenerationException again. Will be written in finally block
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, exception.Message);
                }
                finally
                {
                    if (
                        Logger.Entries.Any(
                            e => e.Severity == LogEntrySeverity.Error || e.Severity == LogEntrySeverity.Fatal))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (Logger.Entries.Any(e => e.Severity == LogEntrySeverity.Warning))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }

                    if (settings != null && !settings.ShowHelp)
                    {
                        if (Logger.Entries.Any(e => e.Severity == LogEntrySeverity.Error || e.Severity == LogEntrySeverity.Fatal))
                        {
                            if (!string.Equals("None", settings.CodeGenerator, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine(Resources.GenerationFailed);
                                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1}",
                                    typeof(Program).Assembly.ManifestModule.Name,
                                    string.Join(" ", args)));
                            }
                        }
                        else
                        {
                            if (!string.Equals("None", settings.CodeGenerator, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine(Resources.GenerationComplete,
                                    settings.CodeGenerator, settings.Input);
                            }
                            exitCode = (int)ExitCode.Success;
                        }
                    }

                    // Write all messages to Console
                    Console.ResetColor();
                    var validationLevel = settings?.ValidationLevel ?? LogEntrySeverity.Error;
                    bool isVerbose = settings?.Verbose ?? false;
                    foreach (var severity in (LogEntrySeverity[])Enum.GetValues(typeof(LogEntrySeverity)))
                    {
                        // Set the color if the severity level has a set console color
                        Console.ForegroundColor = severity.GetColorForSeverity();
                        // Determine if this severity of messages should be treated as errors
                        bool isErrorMessage = severity >= validationLevel;
                        // Set the output stream based on if the severity should be an error or not
                        var outputStream = isErrorMessage ? Console.Error : Console.Out;
                        // If it's an error level severity or we want to see all output, write to console
                        if (isErrorMessage || isVerbose)
                        {
                            Logger.WriteMessages(outputStream, severity, isVerbose);
                        }
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(Resources.ConsoleErrorMessage, exception.Message);
                Console.Error.WriteLine(Resources.ConsoleErrorStackTrace, exception.StackTrace);
            }
            return exitCode;
        }

        /// <summary>
        /// Returns true if show markdown flag is specified among the command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>True if markdown formatted help should be shown, otherwise false.</returns>
        private static bool IsShowMarkdownHelpIncluded(string[] args)
        {
            if (args.Any(a => a == "-md" || "-markdown".Equals(a, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            return false;
        }
    }
}