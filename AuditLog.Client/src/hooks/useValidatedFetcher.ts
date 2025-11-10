import { useCallback } from "react";
import * as v from "valibot";

/**
 * Hook that creates a validated fetcher function for use with SWR
 * @param schema - Valibot schema to validate the response against
 * @returns Fetcher function that validates the response
 */
export const useValidatedFetcher = <T>(schema: v.BaseSchema<unknown, T, v.BaseIssue<unknown>>) => {
  return useCallback(
    async (url: string): Promise<T> => {
      const response = await fetch(url);

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();

      try {
        return v.parse(schema, data);
      } catch (error) {
        if (error instanceof v.ValiError) {
          console.error("Validation error:", error.issues);
          throw new Error(`Response validation failed: ${error.message}`);
        }
        throw error;
      }
    },
    [schema]
  );
};
