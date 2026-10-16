import React, { createContext, useContext, useState, useEffect } from 'react';
import { ALL_TRANSLATIONS, LANGUAGES } from '../locales';
import type { SupportedLanguage, LanguageInfo, TranslationKeys } from '../locales';

interface LanguageContextType {
  language: SupportedLanguage;
  setLanguage: (lang: SupportedLanguage) => void;
  t: (key: TranslationKeys) => string;
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

export const LanguageProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [language, setLanguageState] = useState<SupportedLanguage>('tr');

  useEffect(() => {
    const saved = localStorage.getItem('obs_language') as SupportedLanguage;
    if (saved && ALL_TRANSLATIONS[saved]) {
      setLanguageState(saved);
    }
  }, []);

  const setLanguage = (lang: SupportedLanguage) => {
    setLanguageState(lang);
    localStorage.setItem('obs_language', lang);
  };

  const t = (key: TranslationKeys): string => {
    const currentDict = ALL_TRANSLATIONS[language];
    if (currentDict && currentDict[key]) {
      return currentDict[key];
    }
    // Fallback Türkçe
    return ALL_TRANSLATIONS.tr[key] || (key as string);
  };

  return (
    <LanguageContext.Provider value={{ language, setLanguage, t }}>
      {children}
    </LanguageContext.Provider>
  );
};

export const useLanguage = () => {
  const context = useContext(LanguageContext);
  if (!context) {
    throw new Error('useLanguage must be used within a LanguageProvider');
  }
  return context;
};

export { LANGUAGES };
export type { SupportedLanguage, LanguageInfo, TranslationKeys };
