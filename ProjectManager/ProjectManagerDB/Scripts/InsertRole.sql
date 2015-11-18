BEGIN TRAN

INSERT INTO Role
(user_id, project_id, type)
VALUES
(3, 1, 2),
(1, 1, 1),
(2, 1, 1),
(4, 1, 1)

COMMIT