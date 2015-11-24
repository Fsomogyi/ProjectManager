BEGIN TRAN

INSERT INTO Project
VALUES
		  ('Project Manager',
		   'Projekt menedzselő webes alkalmazás fejlesztése.',
		   '2015-11-30 0:00:00 AM',
		   0, '2015-11-10 0:00:00 AM'),
		  ('Ipsum dolor',
           'Description. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.',
           '2015-11-30 0:00:00 AM',
           0, '2015-11-10 0:00:00 AM'),
		   ('Consectetuer adip',
           'Consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.',
           '2016-6-15 0:00:00 AM',
           1, '2015-11-10 0:00:00 AM'),
		   ('Diam nonummy',
           'Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.',
           '2016-2-1 0:00:00 AM',
           0, '2015-11-15 0:00:00 AM'),
		   ('Tincidunt',
           'Sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Quis nostrud exerci tation ullamcorper.  Ut wisi enim ad minim veniam, suscipit lobortis nisl ut aliquip ex ea commodo consequat.',
           '2015-12-20 0:00:00 AM',
           1, '2015-11-10 0:00:00 AM')

COMMIT
