import React from "react";
import { Progress, Tooltip } from "antd";

type DifficultyProgressProps = {
  level: string;
};

const DifficultyProgress: React.FC<DifficultyProgressProps> = ({ level }) => {
  const config: Record<string, { percent: number; color: string }> = {
    Легкий: { percent: 33, color: "#52c41a" },
    Средний: { percent: 66, color: "#faad14" },
    Сложный: { percent: 100, color: "#f5222d" },
  };

  const { percent, color } = config[level] || {};

  return (
    <div>
      <Tooltip key={level} title={level}>
        <Progress
          percent={percent}
          showInfo={false}
          steps={3}
          size={{ width: 30 }}
          strokeColor={color}
          style={{ width: 100 }}
        />
      </Tooltip>
    </div>
  );
};

export default DifficultyProgress;
