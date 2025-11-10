import * as v from "valibot";

/**
 * Schema for organizations list (array of UUID strings)
 */
export const organizationsSchema = v.pipe(
  v.array(v.pipe(v.string(), v.uuid())),
  v.description("List of organization IDs")
);

export type Organizations = v.InferOutput<typeof organizationsSchema>;
