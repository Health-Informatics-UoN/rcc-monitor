type ReportType = {
  id: number;
  name: string;
};

type Status = {
  id: number;
  name: string;
};

type Instance = {
  id: number;
  name: string;
};

type ReportModel = {
  id: number;
  dateTime: Date;
  siteName: string;
  siteId: string;
  description: string;
  reportType: ReportType;
  instance: Instance;
  status: Status;
};
