﻿BEGIN TRAN

INSERT INTO Role
(UserId, ProjectId, Type)
VALUES
(3, 1, 2),
(1, 1, 1),
(2, 1, 1),
(4, 1, 1),
(1, 4, 2),
(1, 5, 1),
(1, 6, 1)

COMMIT