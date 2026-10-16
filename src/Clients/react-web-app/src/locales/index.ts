import { tr } from './tr';
import { en } from './en';
import { de } from './de';
import { ru } from './ru';
import { ge } from './ge';
import type { TranslationKeys } from './tr';

export type SupportedLanguage = 'tr' | 'en' | 'de' | 'ru' | 'ge';

export interface LanguageInfo {
  code: SupportedLanguage;
  name: string;
  flag: string;
}

export const LANGUAGES: LanguageInfo[] = [
  { code: 'tr', name: 'Türkçe', flag: '🇹🇷' },
  { code: 'en', name: 'English', flag: '🇬🇧' },
  { code: 'de', name: 'Deutsch', flag: '🇩🇪' },
  { code: 'ru', name: 'Русский', flag: '🇷🇺' },
  { code: 'ge', name: 'ქართული', flag: '🇬🇪' },
];

export const ALL_TRANSLATIONS: Record<SupportedLanguage, Record<TranslationKeys, string>> = {
  tr,
  en,
  de,
  ru,
  ge,
};

export type { TranslationKeys };
