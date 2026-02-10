import React from 'react';
import { Tooltip } from 'antd';

type DifficultyScaleProps = {
  level: string;
};

const DifficultyScale: React.FC<DifficultyScaleProps> = ({ level }) => {
  const levels = ['Легкий', 'Средний', 'Сложный'];
  const colors: Record<string, string> = {
    Легкий: '#52c41a',
    Средний: '#faad14',
    Сложный: '#f5222d',
  };

  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
      {levels.map((lvl) => (
        <Tooltip key={lvl} title={lvl}>
          <div
            style={{
              width: 30,
              height: 10,
              borderRadius: 4,
              backgroundColor: lvl === level ? colors[lvl] : '#f0f0f0',
              transition: 'background-color 0.3s',
            }}
          />
        </Tooltip>
      ))}
    </div>
  );
};

export default DifficultyScale;