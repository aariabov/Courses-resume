import React from 'react';
import DifficultyProgress from "./DifficultyProgress";

type DifficultyMeterProps = {
  level: string;
};

const DifficultyMeter: React.FC<DifficultyMeterProps> = ({ level }) => {
  const levels = ['Легкий', 'Средний', 'Сложный'];
  const activeIndex = levels.indexOf(level);
  const colors = ['#52c41a', '#faad14', '#f5222d'];

  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
      {levels.map((_, i) => (
        <div
          key={i}
          style={{
            width: 20 + i * 10,
            height: 10,
            borderRadius: 4,
            backgroundColor: i <= activeIndex ? colors[activeIndex] : '#f0f0f0',
            transition: 'background-color 0.3s',
          }}
        />
      ))}
      <span style={{ fontSize: 12, color: colors[activeIndex], fontWeight: 500 }}>
        {level}
      </span>
    </div>
  );
};

export default DifficultyMeter;
