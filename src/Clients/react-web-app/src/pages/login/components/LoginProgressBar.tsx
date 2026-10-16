import React from 'react';

interface LoginProgressBarProps {
  progress: number; // 0 to 100
  isVisible: boolean;
}

export const LoginProgressBar: React.FC<LoginProgressBarProps> = ({
  progress,
  isVisible,
}) => {
  if (!isVisible) {
    return null;
  }

  return (
    <div
      style={{
        width: '100%',
        height: 2,
        backgroundColor: 'rgba(255, 255, 255, 0.08)',
        position: 'relative',
        overflow: 'hidden',
        zIndex: 15,
        flexShrink: 0,
      }}
    >
      <div
        style={{
          width: `${Math.min(100, Math.max(0, progress))}%`,
          height: '100%',
          background: 'linear-gradient(90deg, rgba(255, 255, 255, 0.2) 0%, rgba(255, 255, 255, 0.45) 100%)',
          boxShadow: 'none',
          transition: progress === 0 ? 'none' : 'width 50ms linear',
        }}
      />
    </div>
  );
};
