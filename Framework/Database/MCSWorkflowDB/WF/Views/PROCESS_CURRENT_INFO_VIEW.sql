
CREATE VIEW [WF].[PROCESS_CURRENT_INFO_VIEW]
AS
SELECT     P.INSTANCE_ID AS PROCESS_ID, P.RESOURCE_ID, P.DESCRIPTOR_KEY, P.PROCESS_NAME, ACI.SUBJECT, P.APPLICATION_NAME, 
                      P.STATUS AS PROCESS_STATUS, P.DEPARTMENT_ID, P.DEPARTMENT_NAME, P.DEPARTMENT_PATH, A.ACTIVITY_ID, A.ACTIVITY_DESC_KEY, A.ACTIVITY_NAME, A.OPERATOR_ID, A.OPERATOR_NAME, A.OPERATOR_PATH, A.STATUS AS CURRENT_STATUS, A.START_TIME, 
                      A.END_TIME, CA.USER_ID AS CURRENT_USER_ID, CA.USER_NAME AS CURRENT_USER_NAME, CA.USER_PATH AS CURRENT_USER_PATH, P.PROGRAM_NAME
FROM         WF.PROCESS_INSTANCES AS P WITH (NOLOCK) INNER JOIN
                      WF.PROCESS_CURRENT_ACTIVITIES AS A WITH (NOLOCK) ON P.INSTANCE_ID = A.PROCESS_ID INNER JOIN
                      WF.PROCESS_CURRENT_ASSIGNEES AS CA WITH (NOLOCK) ON A.ACTIVITY_ID = CA.ACTIVITY_ID LEFT OUTER JOIN
                      WF.APPLICATIONS_COMMON_INFO AS ACI WITH (NOLOCK) ON P.RESOURCE_ID = ACI.RESOURCE_ID


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "P"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 245
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "A"
            Begin Extent = 
               Top = 6
               Left = 283
               Bottom = 125
               Right = 474
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CA"
            Begin Extent = 
               Top = 6
               Left = 512
               Bottom = 125
               Right = 680
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ACI"
            Begin Extent = 
               Top = 6
               Left = 718
               Bottom = 125
               Right = 946
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 17
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortT', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'VIEW', @level1name = N'PROCESS_CURRENT_INFO_VIEW';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'ype = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'VIEW', @level1name = N'PROCESS_CURRENT_INFO_VIEW';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'VIEW', @level1name = N'PROCESS_CURRENT_INFO_VIEW';

