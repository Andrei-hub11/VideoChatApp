export function omit<T extends object, K extends keyof T>(
  obj: T,
  keyToOmit: K,
): Omit<T, K> {
  const { [keyToOmit]: _, ...rest } = obj; // Aqui a variável omitida é nomeada como 'omitted'
  return rest; // Retorna o objeto sem a chave omitida
}
