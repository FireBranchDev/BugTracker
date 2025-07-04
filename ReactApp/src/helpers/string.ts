export const truncateString = (str: string, maxLength: number): string => {
  if (str.length > maxLength) {
    return str.slice(0, maxLength) + '...';
  }

  return str;
};
