import React from 'react';

interface LoginBackgroundProps {
  images: string[];
  currentIndex: number;
  mode: 'slideshow' | 'single' | 'empty';
}

export const LoginBackground: React.FC<LoginBackgroundProps> = ({
  images,
  currentIndex,
  mode,
}) => {
  if (mode === 'empty' || !images || images.length === 0) {
    return (
      <div
        style={{
          position: 'fixed',
          inset: 0,
          background: 'linear-gradient(135deg, #091e3a 0%, #102a4e 45%, #183b6b 100%)',
          zIndex: 0,
        }}
      />
    );
  }

  const imageList = mode === 'single' ? [images[0]] : images;

  return (
    <div
      style={{
        position: 'fixed',
        inset: 0,
        overflow: 'hidden',
        zIndex: 0,
        backgroundColor: '#07162c',
      }}
    >
      {/* Resim Katmanları - Sinematik Cross-Fade & Ken Burns Efekti */}
      {imageList.map((img, idx) => {
        const isActive = mode === 'single' ? true : idx === currentIndex;
        return (
          <div
            key={img}
            style={{
              position: 'absolute',
              inset: -20, // Ken Burns zoom payı
              backgroundImage: `url('${img}')`,
              backgroundSize: 'cover',
              backgroundPosition: 'center',
              opacity: isActive ? 1 : 0,
              zIndex: isActive ? 2 : 1,
              transform: isActive ? 'scale(1.05)' : 'scale(1)',
              transition: 'opacity 1.4s ease-in-out, transform 6s cubic-bezier(0.25, 1, 0.5, 1)',
              willChange: 'opacity, transform',
              filter: 'brightness(0.85) contrast(1.05)',
            }}
          />
        );
      })}

      {/* Kurumsal Lüks Karartma & Gradyan Katmanı (Formun okunurluğu için) */}
      <div
        style={{
          position: 'absolute',
          inset: 0,
          zIndex: 3,
          background:
            'radial-gradient(circle at center, rgba(9, 25, 48, 0.52) 0%, rgba(7, 18, 36, 0.82) 100%), linear-gradient(180deg, rgba(9, 25, 48, 0.85) 0%, rgba(9, 25, 48, 0.38) 40%, rgba(9, 25, 48, 0.38) 60%, rgba(9, 25, 48, 0.9) 100%)',
          pointerEvents: 'none',
        }}
      />
    </div>
  );
};
