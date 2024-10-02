// utils.test.ts
import { describe, it, expect } from "vitest";

import { omit } from "../../utils/helpers/omit";

// Altere o caminho se necessário

describe("omit", () => {
  it("should omit the specified key from the object", () => {
    const obj = { a: 1, b: 2, c: 3 };
    const result = omit(obj, "b");

    expect(result).toEqual({ a: 1, c: 3 });
  });

  it("should return the same object if the key does not exist", () => {
    const obj = { a: 1, b: 2 };
    const result = omit(obj, "c" as keyof typeof obj); // 'c' dot not exists on object

    // The object is expected to remain unchanged
    expect(result).toEqual({ a: 1, b: 2 });
  });

  it("should return an empty object if all keys are omitted", () => {
    const obj = { a: 1, b: 2 };
    const result = omit(obj, "a");

    expect(omit(result, "b")).toEqual({});
  });
});
