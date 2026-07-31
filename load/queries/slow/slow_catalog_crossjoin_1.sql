SELECT COUNT(*) AS Combinations
FROM (SELECT TOP (300) object_id FROM sys.all_objects ORDER BY object_id) AS a
CROSS JOIN (SELECT TOP (300) object_id FROM sys.all_objects ORDER BY object_id) AS b
CROSS JOIN (SELECT TOP (300) object_id FROM sys.all_objects ORDER BY object_id) AS c;
