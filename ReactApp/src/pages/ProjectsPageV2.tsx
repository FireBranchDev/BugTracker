import {
  Box,
  Grid,
  List,
  ListItem,
  Paper,
  styled,
  Typography,
} from '@mui/material';

const Item = styled(Paper)(({ theme }) => ({
  backgroundColor: '#fff',
  ...theme.typography.body2,
  padding: theme.spacing(1),
  textAlign: 'center',
  color: (theme.vars ?? theme).palette.text.secondary,
  ...theme.applyStyles('dark', {
    backgroundColor: '#1A2027',
  }),
}));

const ProjectsPageV2 = () => {
  return (
    <Box sx={{ flexGrow: 1 }}>
      <Typography variant="h1" align="center">
        Projects Page V2
      </Typography>
      <Grid container spacing={3}>
        <Grid size="grow">
          <Item>
            <Typography variant="h3">Your Projects</Typography>
            <List>
              <ListItem>Alpha</ListItem>
              <ListItem>Beta</ListItem>
            </List>
            <Typography variant="h3">Collaborative Projects</Typography>
            <List>
              <ListItem>Charlie</ListItem>
              <ListItem>Delta</ListItem>
            </List>
          </Item>
        </Grid>
        <Grid size={8}>
          <Item>
            <Typography variant="h2">Alpha</Typography>
            <Grid container spacing={3}>
              <Grid size="grow">
                <Item>
                  <Typography fontWeight="bold">Date created:</Typography>
                  <Typography textAlign="left">Today</Typography>
                </Item>
              </Grid>
              <Grid size={10}>
                <Item>
                  <Typography variant="h3">Bugs</Typography>
                </Item>
              </Grid>
            </Grid>
          </Item>
        </Grid>
      </Grid>
    </Box>
  );
};

export default ProjectsPageV2;
