create or replace view v_report_project_issue_count as
SELECT id, mfg_date, (select count(id) from issue_log.issues where root_cause_process_id not in(41,42,43,44,24) 
			and (project = public.projects.id or project_no like CONCAT('%', public.projects.id, '%'))
		   )as number_of_issues from public.projects order by id
-- Start --
-- insert deliverables for line ready project. 
INSERT INTO public.deliverables(
	project_id, status, description, type, suffix_id, is_latenotice, start_delay, end_delay)
	SELECT id, 'Pending','Mechanical drawing cross-check', 'CD', 'CD', true,0,0 FROM public.project_schedule where is_line;
-- End---