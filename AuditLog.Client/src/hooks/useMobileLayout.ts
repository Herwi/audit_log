import { useMediaQuery } from "./useMediaQuery";

export function useMobileLayout(breakpoint: number = 768): boolean {
  return useMediaQuery(`(max-width: ${breakpoint - 1}px)`);
}
