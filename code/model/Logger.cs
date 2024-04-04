using System.Collections.Generic;
public class Logger {
	private static List<LoggerListener> listeners = new List<LoggerListener>();
	public static void subscribe(LoggerListener listener) {
		Logger.listeners.Add(listener);
	}
	public static void logMessage(string message) {
		foreach (LoggerListener listener in Logger.listeners) {
			listener.logMessage(message);
		}
		//TODO for test.
		// System.Console.WriteLine(message);
	}
}
