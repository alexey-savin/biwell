SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ItemWeight](
	[ItemId] [varchar](20) NOT NULL,
	[Weight] [numeric](5, 2) NOT NULL,
 CONSTRAINT [PK_ItemId] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

insert ItemWeght (ItemId, Weight)
values 
('1001', 0.15),
('1003', 4.5),
('1004', 0.19),
('1006', 0.15),
('1007', 0.15),
('1008', 0.15),
('1009', 0.15),
('1010', 0.28),
('1011', 0.08),
('1012', 0.15),
('1013', 0.15),
('1014', 0.15),
('1015', 0.15),
('1016', 0.15),
('1017', 0.19),
('1018', 0.19),
('1019', 0.19),
('1020', 4.5),
('1021', 0.3),
('1023', 5.6),
('1024', 0.0)
