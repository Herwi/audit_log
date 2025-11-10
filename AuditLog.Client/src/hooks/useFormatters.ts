import { useCallback } from "react";

/**
 * Hook that provides formatting utilities for dates and durations
 */
export const useFormatters = () => {
  /**
   * Formats an ISO datetime string to a readable date and time
   * @param isoString - ISO 8601 datetime string
   * @returns Formatted date and time string
   */
  const formatDateTime = useCallback((isoString: string): string => {
    const date = new Date(isoString);
    return new Intl.DateTimeFormat("en-US", {
      year: "numeric",
      month: "short",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      hour12: false,
    }).format(date);
  }, []);

  /**
   * Formats a TimeSpan string (HH:MM:SS.fff format) to a readable duration
   * @param timeSpan - TimeSpan string from backend
   * @returns Formatted duration string (e.g., "5.12s", "1m 30s", "2h 15m")
   */
  const formatDuration = useCallback((timeSpan: string): string => {
    // Parse TimeSpan format: HH:MM:SS.ffffff or similar
    const parts = timeSpan.split(":");
    if (parts.length !== 3) return timeSpan;

    const hours = parseInt(parts[0], 10);
    const minutes = parseInt(parts[1], 10);
    const secondsParts = parts[2].split(".");
    const seconds = parseInt(secondsParts[0], 10);
    const milliseconds = secondsParts[1] ? parseInt(secondsParts[1].substring(0, 3), 10) : 0;

    // Format based on magnitude
    if (hours > 0) {
      return minutes > 0 ? `${hours}h ${minutes}m` : `${hours}h`;
    } else if (minutes > 0) {
      return seconds > 0 ? `${minutes}m ${seconds}s` : `${minutes}m`;
    } else if (seconds > 0) {
      const totalSeconds = seconds + milliseconds / 1000;
      return `${totalSeconds.toFixed(2)}s`;
    } else {
      return `${milliseconds}ms`;
    }
  }, []);

  return {
    formatDateTime,
    formatDuration,
  };
};
