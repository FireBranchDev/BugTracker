import { FC } from 'react';
import * as material from '@mui/material';
import { useTheme } from '@mui/material/styles';

const Features: FC = () => {
  const theme = useTheme();
  return (
    <section id="features">
      <material.Typography variant="h3">Features</material.Typography>
      <material.Box sx={{ marginTop: theme.spacing(2) }}>
        <material.Tabs value={0} aria-label="basic tabs example">
          <material.Tab label="Item One" />
          <material.Tab label="Item Two" />
          <material.Tab label="Item Three" />
        </material.Tabs>
      </material.Box>
    </section>
  );
};

export default Features;
